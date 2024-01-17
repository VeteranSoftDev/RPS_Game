pub mod utils;
use borsh::{BorshDeserialize,BorshSerialize};
use {
    crate::utils::*,
    anchor_lang::{
        prelude::*,
        AnchorDeserialize,
        AnchorSerialize,
        Key,
        solana_program::{
            program_pack::Pack,
            msg
        }      
    },
    spl_token::state,
};
declare_id!("Fg6PaFpoGXkYsidMpWTK6W2BeZ7FEfcYkg476zPFsLnS");

#[program]
pub mod solana_anchor {
    use super::*;

    pub fn init_pool(
        ctx : Context<InitPool>,
        _bump : u8,
        _fee : u64,
        ) -> ProgramResult {
        msg!("+ init_pool");
        let pool = &mut ctx.accounts.pool;
        let token_account : state::Account = state::Account::unpack_from_slice(&ctx.accounts.token_account.data.borrow())?;
        if token_account.owner != pool.key(){
            msg!("Owner of token account must be pool");
            return Err(PoolError::InvalidTokenAccount.into());
        }
        if token_account.mint != *ctx.accounts.token_mint.key{
            msg!("Mint of token account must be token_mint");
            return Err(PoolError::InvalidTokenAccount.into());
        }
        pool.owner = *ctx.accounts.owner.key;
        pool.rand = *ctx.accounts.rand.key;
        pool.token_mint = *ctx.accounts.token_mint.key;
        pool.token_account = *ctx.accounts.token_account.key;
        pool.fee = _fee;
        pool.change_amount = 0;
        pool.bump = _bump;
        Ok(())
    }

    pub fn init_player(
        ctx : Context<InitPlayer>,
        _bump : u8
        ) -> ProgramResult {
        msg!("+ init_player");
        let player = &mut ctx.accounts.player;
        player.owner = *ctx.accounts.owner.key;
        player.pool = ctx.accounts.pool.key();
        player.amount = 0;
        player.bump = _bump;
        Ok(())
    }

    pub fn deposit(
        ctx : Context<Deposit>,
        _amount : u64,
        ) ->ProgramResult {
        msg!("+ deposit");
        let pool = &ctx.accounts.pool;
        let player = &mut ctx.accounts.player;
        if pool.token_account != *ctx.accounts.token_to.key {
            msg!("token_to must be token_account of pool");
            return Err(PoolError::InvalidTokenAccount.into());
        }
        spl_token_transfer(
            TokenTransferParams{
                source : ctx.accounts.token_from.clone(),
                destination : ctx.accounts.token_to.clone(),
                authority : ctx.accounts.transfer_authority.clone(),
                authority_signer_seeds : &[],
                token_program : ctx.accounts.token_program.clone(),
                amount : _amount,
            }
        )?;        
        player.amount = player.amount + _amount;
        Ok(())
    }

    pub fn withdraw(
        ctx : Context<Withdraw>,
        _amount : u64,
        ) ->ProgramResult {
        msg!("+ withdraw");
        let pool = &mut ctx.accounts.pool;
        let player = &mut ctx.accounts.player;
        if pool.token_account != *ctx.accounts.token_from.key {
            msg!("token_from must be token_account of pool");
            return Err(PoolError::InvalidTokenAccount.into());
        }
        if player.amount < _amount {
            msg!("Your deposit amount is smaller than _amount");
            return Err(PoolError::InvalidAmount.into());
        }
        let pool_seeds = &[pool.rand.as_ref(),&[pool.bump]];
        spl_token_transfer(
            TokenTransferParams{
                source : ctx.accounts.token_from.clone(),
                destination : ctx.accounts.token_to.clone(),
                authority : pool.to_account_info().clone(),
                authority_signer_seeds : pool_seeds,
                token_program : ctx.accounts.token_program.clone(),
                amount : _amount,
            }
        )?;
        player.amount = player.amount - _amount;                
        Ok(())
    }

    pub fn redeem_token(
        ctx : Context<RedeemToken>,
        _amount : u64
        )->ProgramResult{
        msg!("+ redeem_token");
        let pool = &mut ctx.accounts.pool;
        if *ctx.accounts.token_from.key != pool.token_account {
            msg!("token_from need to be token_account of pool");
            return Err(PoolError::InvalidTokenAccount.into());
        }
        let pool_seeds = &[pool.rand.as_ref(),&[pool.bump]];        
        spl_token_transfer(
            TokenTransferParams{
                source : ctx.accounts.token_from.clone(),
                destination : ctx.accounts.token_to.clone(),
                authority : pool.to_account_info().clone(),
                authority_signer_seeds : pool_seeds,
                token_program : ctx.accounts.token_program.clone(),
                amount : _amount,
            }
        )?;

        Ok(())
    }

    pub fn update_pool(
        ctx : Context<UpdatePool>,
        _fee : u64,
        ) ->ProgramResult {
        msg!("+ update_pool");
        let pool = &mut ctx.accounts.pool;
        pool.fee = _fee;
        Ok(())
    }

    pub fn play(
        ctx : Context<Play>,
        _amount : u64,
        ) -> ProgramResult {
        msg!("+ play");
        let pool = &mut ctx.accounts.pool;
        let player = &mut ctx.accounts.player;
        if player.amount < _amount {
            msg!("Your deposit amount is smaller than _amount");
            return Err(PoolError::InvalidAmount.into());
        }
        let clock = (Clock::from_account_info(&ctx.accounts.clock)?).unix_timestamp as u64;
        
        if clock % 2 == 1 {
            pool.change_amount += _amount as i64;
            player.amount -= _amount;
        } else {
            let fee_amount = _amount * pool.fee / 1000;
            player.amount += _amount - fee_amount;
            pool.change_amount -= (_amount - fee_amount) as i64;
        }
        
        Ok(())
    }
}

#[derive(Accounts)]
pub struct Play<'info>{
    #[account(mut, signer)]
    owner : AccountInfo<'info>,

    #[account(mut)]
    pool : ProgramAccount<'info,Pool>,

    #[account(mut, seeds=[(*owner.key).as_ref(),pool.key().as_ref()], bump=player.bump)]
    player : ProgramAccount<'info, Player>,

    clock : AccountInfo<'info>,       
}

#[derive(Accounts)]
pub struct UpdatePool<'info>{
    #[account(mut, signer)]
    owner : AccountInfo<'info>,

    #[account(mut,has_one=owner)]
    pool : ProgramAccount<'info,Pool>,

    // #[account(owner=spl_token::id())]
    // token_account : AccountInfo<'info>,    
}

#[derive(Accounts)]
pub struct RedeemToken<'info>{
    #[account(mut, signer)]
    owner : AccountInfo<'info>,

    #[account(mut,has_one=owner)]
    pool : ProgramAccount<'info,Pool>,

    #[account(mut,owner=spl_token::id())]
    token_from : AccountInfo<'info>,

    #[account(mut,owner=spl_token::id())]
    token_to : AccountInfo<'info>,

    #[account(address=spl_token::id())]
    token_program : AccountInfo<'info>,    
}

#[derive(Accounts)]
pub struct Withdraw<'info>{
    #[account(mut, signer)]
    owner : AccountInfo<'info>,

    #[account(mut)]
    pool : ProgramAccount<'info,Pool>,

    #[account(mut, seeds=[(*owner.key).as_ref(),pool.key().as_ref()], bump=player.bump)]
    player : ProgramAccount<'info, Player>,

    #[account(mut, owner=spl_token::id())]
    token_from : AccountInfo<'info>,

    #[account(mut, owner=spl_token::id())]
    token_to : AccountInfo<'info>,    

    #[account(address=spl_token::id())]
    token_program : AccountInfo<'info>,    
}

#[derive(Accounts)]
pub struct Deposit<'info>{
    #[account(mut, signer)]
    owner : AccountInfo<'info>,

    pool : ProgramAccount<'info, Pool>,

    #[account(mut, seeds=[(*owner.key).as_ref(),pool.key().as_ref()], bump=player.bump)]
    player : ProgramAccount<'info, Player>,

    #[account(mut, owner=spl_token::id())]
    token_from : AccountInfo<'info>,

    #[account(mut, owner=spl_token::id())]
    token_to : AccountInfo<'info>,

    #[account(mut,signer)]
    transfer_authority : AccountInfo<'info>,

    #[account(address=spl_token::id())]
    token_program : AccountInfo<'info>,  
}

#[derive(Accounts)]
#[instruction(_bump : u8)]
pub struct InitPlayer<'info>{
    #[account(mut, signer)]
    owner : AccountInfo<'info>,

    pool : ProgramAccount<'info, Pool>,

    #[account(init, seeds=[(*owner.key).as_ref(),pool.key().as_ref()], bump=_bump, payer=owner, space=8+PLAYER_SIZE)]
    player : ProgramAccount<'info, Player>,

    system_program : Program<'info, System>
}

#[derive(Accounts)]
#[instruction(_bump : u8)]
pub struct InitPool<'info>{
    #[account(mut, signer)]
    owner : AccountInfo<'info>,

    #[account(init,seeds=[(*rand.key).as_ref()], bump=_bump, payer=owner, space=8+POOL_SIZE)]
    pool : ProgramAccount<'info,Pool>,

    rand : AccountInfo<'info>,

    #[account(owner=spl_token::id())]
    token_mint : AccountInfo<'info>,

    #[account(owner=spl_token::id())]
    token_account : AccountInfo<'info>,

    system_program : Program<'info, System>
}

pub const POOL_SIZE : usize = 32 + 32 + 32 + 32 + 8 + 8 + 1;
pub const PLAYER_SIZE : usize = 32 + 32 + 8 + 1;

#[account]
pub struct Pool{
    pub owner : Pubkey,
    pub rand : Pubkey,
    pub token_mint : Pubkey,
    pub token_account : Pubkey,
    pub fee : u64,
    pub change_amount : i64,
    pub bump : u8
}

#[account]
pub struct Player{
    pub owner : Pubkey,
    pub pool : Pubkey,
    pub amount : u64,
    pub state : u8,
    pub bump : u8,
}

#[error]
pub enum PoolError {
    #[msg("Token mint to failed")]
    TokenMintToFailed,

    #[msg("Token set authority failed")]
    TokenSetAuthorityFailed,

    #[msg("Token transfer failed")]
    TokenTransferFailed,

    #[msg("Token burn failed")]
    TokenBurnFailed,

    #[msg("Invalid token account")]
    InvalidTokenAccount,

    #[msg("Token not matched")]
    TokenNotMatched,

    #[msg("Not enough amount")]
    NotEnoughAmount,

    #[msg("Invalid owner")]
    InvalidOwner,

    #[msg("Invalid amount")]
    InvalidAmount
}